# Blagoevgrad neighbourhood outlines

Updated 2026-11-07 from eleven screenshots supplied by the user.

The numbered reference-image regions and global west/south offset have been removed.
The current dataset contains Gramada, Southern Industrial Zone, Zapad, Balarbashi,
Osvobozhdenie, City Centre, Orlova Chuka, Alen Mak, Elenovo 1, Elenovo 2, and Strumsko.

Method:
- Digitise the red/white neighbourhood outline in each supplied screenshot.
- Align overlapping satellite imagery using shared ground features with RANSAC similarity transforms.
  The image-pair registration residuals were about 0.6–1.6 pixels.
- Align the common image frame to Web Mercator using seven OSM landmarks:
  City Hotel (node 13660759556), Diva Hotel (node 13660759554), Pulse Hospital
  (way 665030004), Hotel Chamishki (node 4999255467), Hotel Fenix (way 788150463),
  Kaufland (way 611995148), and Build (node 13647412701).
  The landmark fit residuals range from about 2–60 metres. Landmark icon placement,
  building centroids, perspective, and tracing introduce further uncertainty.
- Exclude Gramada from the surrounding industrial zone and resolve overlaps along
  the larger Centre and Industrial outlines against the focused residential screenshots.
- Place each name at an interior point, including concave polygons.

Limitations:
These are screenshot-derived project outlines, not surveyed or official boundaries.
City Centre is cropped at the north and covered by a popup on its eastern side.
Southern Industrial Zone is partly covered by a popup on its western side.
Those obscured sections are estimates. ReferenceImage and BoundaryNote in the JSON
record provenance and limitations per neighbourhood. All records remain IsApproximate=true.
The application embeds the coordinates, so the source screenshots are not needed at runtime.

Base map and coordinate references: © OpenStreetMap contributors (ODbL).
Screenshot imagery is not bundled or used as a tile source; it remains user-supplied reference material.

## User refinements — 2026-11-07

Five new OSM screenshots were registered directly against already cached map tiles.
Registration used 54–511 inlier feature matches per image, with residuals below 0.8 tile pixels.
These refinements change selected boundaries; they do not shift the whole map layer:
- Zapad: southwest edge follows the boulevard and includes the displayed blocks 36 through 29.
- Gramada: widen the upper west edge to include the houses between the former outline and the street.
- Strumsko: clip the east edge to the west side of Route 1, excluding the main road corridor.
- Alen Mak: expand the west edge to include the apartment buildings shown to the left.
- Elenovo 1 and 2: move north together by 0.00045 degrees latitude (about 50 metres).
Larger surrounding polygons yield to the refined residential outlines to avoid conflicting selections.
Each refined record includes AdjustmentReference identifying the supplied screenshot.

## University and Varosha additions — 2026-11-07

- Southern Industrial Zone now includes the full South-West University campus footprint
  from the cached OSM way 432571314. A narrow project-zone connection spans the roughly
  17.5-metre road gap between the campus and the existing polygon.
- Varosha is a separate neighbourhood traced from the user screenshot
  codex-clipboard-a1a08395-f776-4db7-ba31-d00baa7dcf87.png. It was registered to the
  previously aligned satellite screenshots using 571 inlier shared features.
- Varosha is removed from the broader City Centre/Industrial outlines so clicks have
  a distinct neighbourhood target. All outlines remain project geometry, not official boundaries.

## Numbered building map — 2026-11-07

Water Meters uses a separate map with no zone polygons. Buildings.json is a public
OpenStreetMap snapshot covering longitude 23.063–23.122 and latitude 41.983–42.032.
The seven bounded API source URLs and retrieval time are recorded in the JSON.
Building data: © OpenStreetMap contributors, Open Database License 1.0
(https://www.openstreetmap.org/copyright and https://opendatacommons.org/licenses/odbl/1-0/).

The snapshot contains 5,564 mapped building records. Closed building ways and
building multipolygon relations retain their WKT footprints and an interior point,
so courtyard/L-shaped buildings keep their pins inside the footprint. Relation
member ways are counted once. Building nodes inside an already mapped footprint
are excluded; isolated building nodes remain. Building parts are not separately
included unless they are explicitly tagged as buildings. All retrieved building
footprints were complete. Raw API data and source screenshots are not needed at runtime.

At runtime, Neighbourhoods.json polygons and holes are intersected with the building
footprints using NetTopologySuite. A building qualifies when a positive-area part
of its footprint lies in a zone; a point/line boundary touch alone does not qualify.
The current zones contain 4,822 unique building records. The map assigns consecutive
numbers from 1 in OSM element-type/ID order. A shared boundary or a region filter
does not duplicate or renumber a building. Numbers are stable for this snapshot
and zone geometry; a later data refresh or zone edit can change the sequence.
These numbers identify map buildings, not installed meters or database records.

Existing in-zone pin positions are retained. If a building's original interior
point lies outside every zone, the pin moves to an interior point of its largest
in-zone footprint portion. A building crossing multiple zones appears in their
filters with the same global number and has one pin in the combined map.
Standalone OSM building nodes continue to use point-in-zone selection.

The map starts centered on Blagoevgrad. Every qualifying building has a pin;
compact pins are used at city zoom and numbers appear from zoom 17. Hover gives
the number, region, name/address where available. Click selects a pin; Building #
and Go find a specific number. Only drawing objects in/near the viewport are
created, while the complete numbered list remains available for filtering/search.

Coverage depends on OSM mapping and the project's approximate zones. Unmapped
physical buildings cannot be inferred from this dataset. Refresh tooling is in
Tools/refresh_building_data.py (requires Python and Shapely); rebuild the project
after refreshing the embedded JSON. The script caches API extracts between runs.


## Strumsko residential expansion — 2026-11-08

The user reference codex-clipboard-24344320-b47d-476a-99d7-2f8285cbed22.png
was aligned with cached OSM tiles using 51 feature matches (0.81-pixel residual).
The western residential frontage and southern housing edge were expanded to
include the previously excluded houses. Existing Strumsko area was preserved;
the Route 1 corridor remains outside. The adjoining Southern Industrial Zone
yields about 4,670 square metres at the shared edge so region selection stays unique.
The outline remains an approximate project boundary.

Strumsko now includes 571 mapped building records, up from 363 (208 additional
records). Three of those buildings previously belonged to the Industrial Zone,
so the city-wide unique numbered total increases by 205, to 4,491.
A small additional OSM API strip extends building coverage south to latitude
41.983; source URLs and individual retrieval times remain in Buildings.json.
The numbered map uses the revised zone geometry automatically.


## Alen Mak red-outline correction — 2026-11-08

The user reference codex-clipboard-69ce1923-815b-434a-a43e-276ca21ab443.png
was aligned with cached OSM tiles using 231 feature matches (0.05-pixel residual).
Alen Mak follows the centre of the user's red drawing, including the school,
the northern apartment frontage up to the road, and Byalata Visota on the west.
It ends on the north side of Peyo Yavorov Boulevard. This remains an approximate
project boundary rather than an official neighbourhood boundary.

The buildings on the south side of the boulevard in the supplied reference now
belong to Southern Industrial Zone. The shared eastern frontage is assigned to
Elenovo 1 as Alen Mak ends on the west side of Simeon Radev Street. A short
connection across the boulevard at the west end keeps Industrial Zone continuous.
A roughly 2-square-metre tracing sliver with no buildings was discarded.

The three adjacent outlines are valid and do not overlap. Every previously zoned
building stays assigned to a zone. Current mapped-building counts: Alen Mak 80,
Southern Industrial Zone 523, Elenovo 1 191; city-wide unique numbered total 4,552.
Other neighbourhoods, including the expanded Strumsko boundary, are unchanged.


## Industrial southern frontage — 2026-11-08

The user reference codex-clipboard-0beeea58-328c-46f4-85cb-80f1bfc7e681.png
was aligned with cached OSM tiles using 430 feature matches (0.02-pixel residual).
The southern boundary beneath Alen Mak now follows the centre of the red stroke
in place of the straight lower edge. The frontage also extends east to the end
of the supplied red line, yielding to Elenovo 1 on its northern side.
Only Southern Industrial Zone changed; other neighbourhood boundaries are intact.
The university campus and the earlier Alen Mak/Strumsko adjustments are preserved.
The outline remains an approximate project boundary.

Industrial Zone now contains 534 mapped building records; the city-wide unique
numbered total is 4,563. Pins are filtered against the revised boundary at runtime.


## Elenovo road boundaries — 2026-11-08

The reference codex-clipboard-9136b4fb-71ea-4ff4-b3f2-199b968b327b.png was
aligned with cached OSM tiles using 123 feature matches (0.73-pixel residual).
Elenovo 1 now ends at the northern road drawn below Orlova Chuka and follows
the marked western frontage. Its unmarked southern edge is preserved.
The former northern protrusion is assigned to adjoining Orlova Chuka.
Elenovo 2 extends north to the marked road, follows the eastern roadside, and
retains its unmarked southern outline. Both areas use the same shared boundary.
The adjoining Industrial outline yields at the shared edges to prevent overlaps.
Alen Mak and all other neighbourhoods retain their previous geometry.
These remain approximate project areas rather than official boundaries.

Current mapped-building counts: Elenovo 1 186, Elenovo 2 150, Orlova Chuka 501,
Southern Industrial Zone 530. The unique numbered total is 4,585.
Two previously included building representative points are outside the revised
road boundaries; all pin memberships are recalculated from the new polygons.


## Elenovo 1 apartment-block inclusion — 2026-11-08

The reference codex-clipboard-b8a2c311-2c60-4fff-8d42-e3c7595babaf.png was
aligned with cached OSM tiles using 238 feature matches (0.07-pixel residual).
Elenovo 1 expands along the red western frontage beside 14-ti Polk and the
park-side apartment blocks, and along the northern frontage beneath Zdravets.
The expansion is unioned with the previous outline: the southern/shared road
boundaries and every previously included Elenovo 1 building are preserved.
Orlova Chuka and Industrial Zone yield at the adjoining edges; other areas,
including Elenovo 2, Alen Mak and Strumsko, are unchanged.
The outlines remain approximate project geometry.

Elenovo 1 now contains 189 mapped building records (previously 186), including
one transferred from Orlova Chuka and two newly covered records. Orlova Chuka
contains 500 records; Industrial Zone remains at 530. The unique numbered total
is 4,587. No previously zoned building is lost.


## Orlova Chuka hole and Varosha road — 2026-11-08

The reference codex-clipboard-92e928b0-b98c-4cc5-8b1d-9f223bd8ae04.png was
aligned with cached OSM tiles using 868 feature matches (0.10-pixel residual).
The internal 11-vertex cutout in Orlova Chuka is filled. Its former area now
belongs to Orlova Chuka and receives the same blue fill as the surrounding area.
Varosha and Orlova Chuka now share the same road boundary from the 14-ti Polk
junction toward the zoo entrance, following the road indicated by the red arrow.
This replaces the previous pair of offset edges and closes the uncovered strip.
Only these two neighbourhoods change; all other recent refinements are retained.
The boundaries remain approximate project outlines.

Orlova Chuka now contains 532 mapped building records, Varosha 359; the unique
numbered total is 4,608. No previously zoned building is lost.


## Riverside building groups — 2026-11-08

The reference codex-clipboard-5063c2a9-602b-46c5-be53-ac7af3857b6d.png was
aligned with cached OSM tiles using 510 feature matches (0.05-pixel residual).
The upper red-outlined group around Orceto is included in City Centre.
The lower campus buildings along the river are included in Southern Industrial
Zone, with the river-side boundary following the user's long red stroke.
The adjacent Varosha edge yields to the upper group. The later road boundary
between Varosha and Orlova Chuka and the filled Orlova hole are preserved.
Only Centre, Industrial and Varosha change; the outlines remain approximate
project areas rather than official boundaries.

Current mapped-building counts: City Centre 1,553, Industrial Zone 536, Varosha
351. The unique numbered total is 4,631. All previously zoned buildings remain
assigned to a zone. Runtime pins use the updated boundaries automatically.


## Centre riverfront and northern neighbourhood — 2026-11-08

Two user references extend City Centre while preserving its previous area:
- codex-clipboard-f66f2999-46fe-4c9b-b039-58a552192561.png: the western riverfront
  buildings beside Tsar Ivan Shishman, aligned using 966 matches (0.08-pixel residual).
- codex-clipboard-88c99dac-b09d-48ee-858b-d85ec00e1970.png: northern streets and
  aquapark frontage, aligned using 809 matches (0.08-pixel residual).
The red strokes are traced through their centres and registered to the basemap.
Each open stroke is closed through the existing Centre interior, so their union
adds the marked areas without removing earlier Centre refinements. The adjoining
Varosha edge yields about 2,872 square metres along the river; no Varosha building
record is removed. Other neighbourhoods and the recent Industrial/campus changes
retain their geometry. AdjustmentReferences lists both supplied images.
These remain approximate project boundaries.

City Centre now contains 1,633 mapped building records (80 more); Varosha remains
at 351. The unique numbered total is 4,711. No previously zoned building is lost.


## Centre frontages and straight Varosha/Industrial edge — 2026-11-08

The reference codex-clipboard-3658f963-5e9f-407a-9c22-cafe61aaf5e0.png was
aligned with cached OSM tiles using 2,113 matches (0.04-pixel residual).
The red-marked frontages along Slavyanska and Matitsa are added to City Centre.
Zapad yields at the Matitsa edge, including four mapped building records.
The adjoining university campus location remains in Industrial Zone.

Varosha and Southern Industrial Zone now share a single straight segment from
the Centre junction to the Orlova road junction. The previous bent/offset edges
are replaced together, filling the small road-side gaps between them. Varosha's
Centre-facing curve and its road boundary toward the zoo are retained.
Other neighbourhoods, including Osvobozhdenie and Orlova Chuka, retain their
geometry. The boundaries remain approximate project areas.

Current mapped-building counts: City Centre 1,673; Zapad 63; Varosha 352;
Industrial Zone 538. The unique numbered total is 4,750. All previously zoned
buildings retain a zone assignment.


## Osvobozhdenie western frontage — 2026-11-08

The reference codex-clipboard-03b4c38e-1670-4cab-b202-6bb49f40ea2a.png was
aligned with cached OSM tiles using 967 feature matches (0.09-pixel residual).
Osvobozhdenie expands along the centre of the user's red stroke to include the
western residential buildings and Vetren frontage. Its former area is preserved.
The adjoining Centre edge yields about 376 square metres along the junction;
no Centre building record changes zone. Other neighbourhoods, including the
Industrial/university area and straight Varosha boundary, retain their geometry.
These remain approximate project outlines rather than official boundaries.

Osvobozhdenie now contains 134 mapped buildings (42 more); the unique numbered
total is 4,792. Every previously zoned building keeps a zone assignment.


## Complete Water Meters footprint audit — 2026-11-08

All seven bounded public OSM API extracts were downloaded again. The resulting
5,564 deduplicated building records match the previous source dataset, and all
way/relation footprints are complete. Buildings.json now retains FootprintWkt
for each polygon/multipolygon; the refresh tool preserves it on future updates.

The old centre-point-only selector missed 30 buildings whose footprints intersect
a project zone while their representative points fall outside every zone.
NetTopologySuite 2.6.0 now computes positive-area footprint/zone intersections,
including zone holes and building courtyards. Pure edge/point touches are excluded
using a 1e-14 square-degree numerical tolerance. Every qualifying OSM element
still receives one global consecutive number. Buildings spanning zones appear
in the relevant filters with the same number, without duplicate global pins.

All 4,792 previous records and positions are retained. The 30 new pins are anchored
inside their largest in-zone footprint portion. The unique numbered total is
4,822. Current filter memberships are: Alen Mak 81, Balarbashi 98, City Centre 1,701,
Elenovo 1 195, Elenovo 2 155, Gramada 439, Orlova Chuka 545, Osvobozhdenie 144,
Southern Industrial Zone 567, Strumsko 577, Varosha 356, Zapad 68. Membership
counts can overlap because a physical building can cross a project boundary.

An independent geometry comparison checked every runtime ID, zone membership
and pin position against the source footprints. All pins lie inside their
building and at least one assigned zone. All 30 added pins passed number search
and rendering checks; all 12 region filters passed. The separate Loggers map
and existing neighbourhood selections passed their regression checks.
