"""Fetch a bounded OSM snapshot and derive one interior point per building.

Run with Python and Shapely. Raw API responses are cached, never map tiles.
Use --cache to choose a cache folder; delete old extracts there to download a fresh snapshot.
Requires shapely for multipolygon assembly and interior point selection.
"""
import json
import time
import urllib.request
import xml.etree.ElementTree as ET
from datetime import datetime, timezone
from pathlib import Path
from shapely.geometry import Point, Polygon
from shapely.ops import polygonize, unary_union
from shapely.geometry import LineString

import argparse
import tempfile
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--cache', type=Path, default=Path(tempfile.gettempdir()) / 'WaterNetworkMonitoring' / 'building-osm')
args = parser.parse_args()
cache = args.cache
cache.mkdir(parents=True, exist_ok=True)
output = Path(__file__).resolve().parents[1] / 'WaterNetworkMonitoring' / 'Data' / 'Maps' / 'Buildings.json'
nodes, ways, relations = {}, {}, {}
sources = []
requests = [(f'{x}-{y}', west, south, east, north)
            for x, (west, east) in enumerate([(23.063, 23.092), (23.092, 23.122)])
            for y, (south, north) in enumerate([(41.984, 42.000), (42.000, 42.016), (42.016, 42.032)])]
# Include the southern housing edge added to Strumsko on 2026-11-08.
requests.append(('south-extension', 23.063, 41.983, 23.122, 41.984))
for key, west, south, east, north in requests:
    bbox = f'{west},{south},{east},{north}'
    url = f'https://api.openstreetmap.org/api/0.6/map?bbox={bbox}'
    target = cache / f'{key}.osm'
    if not target.exists():
        print('Downloading', bbox, flush=True)
        request = urllib.request.Request(url, headers={'User-Agent': 'WaterNetworkMonitoring/1.0 (student building map)', 'Accept-Encoding': 'identity'})
        with urllib.request.urlopen(request, timeout=100) as response:
            raw = response.read()
        ET.fromstring(raw)
        target.write_bytes(raw)
        time.sleep(1)
    root = ET.parse(target).getroot()
    sources.append({'Url': url, 'RetrievedAtUtc': datetime.fromtimestamp(target.stat().st_mtime, timezone.utc).isoformat(),
                    'OsmBaseTimestamp': root.find('meta').get('osm_base') if root.find('meta') is not None else None})
    for element in root:
        collection = {'node': nodes, 'way': ways, 'relation': relations}.get(element.tag)
        if collection is not None:
            collection[int(element.get('id'))] = element

def tags(element):
    return {t.get('k'): t.get('v') for t in element.findall('tag')}

def line(way):
    refs = [int(n.get('ref')) for n in way.findall('nd')]
    if not all(n in nodes for n in refs):
        return None
    return [(float(nodes[n].get('lon')), float(nodes[n].get('lat'))) for n in refs]

def is_building(t):
    return t.get('building', 'no') not in ('no', 'demolished', 'razed')

buildings = []
footprints = []
consumed = set()
incomplete = []
def add(kind, identifier, t, geometry):
    if geometry.is_empty:
        return
    if not geometry.is_valid:
        geometry = geometry.buffer(0)
    if geometry.is_empty:
        return
    # A representative point stays inside L-shaped buildings and outside courtyards.
    point = geometry.representative_point()
    footprints.append(geometry)
    address = ' '.join(filter(None, [t.get('addr:street'), t.get('addr:housenumber')]))
    buildings.append({'OsmType': kind, 'OsmId': identifier,
                      'Latitude': round(point.y, 7), 'Longitude': round(point.x, 7),
                      'Name': t.get('name:en', t.get('name', '')),
                      'Address': address, 'BuildingType': t['building']})

for identifier, relation in sorted(relations.items()):
    t = tags(relation)
    if not is_building(t) or t.get('type') != 'multipolygon':
        continue
    outer, inner, member_ids = [], [], []
    for m in relation.findall('member'):
        if m.get('type') != 'way':
            continue
        way = ways.get(int(m.get('ref')))
        coords = line(way) if way is not None else None
        if coords is None:
            incomplete.append(f'relation/{identifier}')
            break
        member_ids.append(int(m.get('ref')))
        (inner if m.get('role') == 'inner' else outer).append(LineString(coords))
    else:
        geometry = unary_union(list(polygonize(outer)))
        if inner:
            geometry = geometry.difference(unary_union(list(polygonize(inner))))
        if not geometry.is_empty:
            add('relation', identifier, t, geometry)
            consumed.update(member_ids)

for identifier, way in sorted(ways.items()):
    t = tags(way)
    if identifier in consumed or not is_building(t):
        continue
    coords = line(way)
    if coords is None or len(coords) < 4 or coords[0] != coords[-1]:
        incomplete.append(f'way/{identifier}')
        continue
    add('way', identifier, t, Polygon(coords))

for identifier, node in sorted(nodes.items()):
    t = tags(node)
    if is_building(t):
        point = Point(float(node.get('lon')), float(node.get('lat')))
        if any(footprint.covers(point) for footprint in footprints):
            continue
        # Buildings represented as nodes are legitimate OSM building records too.
        buildings.append({'OsmType': 'node', 'OsmId': identifier,
                          'Latitude': float(node.get('lat')), 'Longitude': float(node.get('lon')),
                          'Name': t.get('name:en', t.get('name', '')), 'Address': '', 'BuildingType': t['building']})

buildings.sort(key=lambda b: (b['OsmType'], b['OsmId']))
result = {'RetrievedAtUtc': max(source['RetrievedAtUtc'] for source in sources),
          'Bounds': [23.063, 41.983, 23.122, 42.032],
          'Attribution': '© OpenStreetMap contributors, ODbL 1.0',
          'Sources': sources, 'Buildings': buildings, 'IncompleteElements': sorted(set(incomplete))}
output.write_text(json.dumps(result, ensure_ascii=False, separators=(',', ':')), encoding='utf-8')
print('Buildings:', len(buildings), 'incomplete:', len(set(incomplete)), flush=True)
print('Incomplete elements:', sorted(set(incomplete)), flush=True)
