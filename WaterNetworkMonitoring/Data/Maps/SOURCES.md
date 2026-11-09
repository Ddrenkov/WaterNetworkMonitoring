# Zapad map data

The project currently uses only Zapad in Blagoevgrad. Neighbourhoods.json contains
one approximate project boundary, and Buildings.json retains only the 68 OSM
building records that intersect that boundary. Other region outlines and building
records have been removed from the embedded map data.

Building data: © OpenStreetMap contributors, Open Database License 1.0.
https://www.openstreetmap.org/copyright
The original API retrieval URLs and dates remain in Buildings.json as provenance.
The outline is based on the supplied project screenshots and refinements; it is
not an official or surveyed administrative boundary.

Water Meters displays numbers 1–68 in OSM element-type/ID order. Footprints must
have positive-area overlap with Zapad; standalone building nodes must lie inside.
Pins are anchored inside their building's in-zone portion when necessary.
The map opens and resets to Zapad. Hover and click show only the building number.
Loggers uses the same single Zapad outline. The general Dashboard basemap remains.

ZapadWaterMeterSeed.cs uses the same numbers 1–68. The six-digit database IDs and
existing readings/dates are preserved. The original AddZapadWaterMeters migration
now inserts map numbers 1–68 when first applied. Rewriting an already-applied
migration does not update existing database rows; those require a separate data update.

Tools/refresh_building_data.py fetches a bounded Zapad extract and filters it to
this same boundary. It cannot restore other regions to Buildings.json. Refreshing
OSM data can change building membership/order; review map-number mappings before
updating an existing database after a future refresh.
