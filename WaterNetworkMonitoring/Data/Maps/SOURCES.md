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
