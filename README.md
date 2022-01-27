# CSVContentMerger
This is a tool created while writing my master thesis and evaluating some data from a miroboard (https://miro.com/). The export function created a table where all data was in one column and separated with two line breaks. 

This tool merges csv files that are separated by "," or ";" or two line breaks. **Note that the two line break merge function currently only works if a merge file (contains all csv data merged together) exists and has the column names that you want merged** (see https://github.com/torantie/CSVContentMerger/blob/8aed71f8e40d48b3d0f064789b856e83cdca3d4c/CSV_Examples/Line_breaks/All_Data.CSV). This was used too look for files missing certain column names (e.g. one csv file doe not have column name "favorite animal").
