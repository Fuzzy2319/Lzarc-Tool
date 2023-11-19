# Lzarc Tool

Usage: Lzarc_Tool [-l/--list] [-x/--extract] [-p/--pack] ...

-l/--list: list the files in given archive
Ex: Lzarc_Tool -l ./Fld_AD_Town_map.lzarc
Ex: Lzarc_Tool --list ./Fld_AD_Town_map.lzarc
-x/--extract: extract the files from an arc hive to the given directory
Ex: Lzarc_Tool -x ./Fld_AD_Town_map.lzarc ./Fld_AD_Town_map/
Ex: Lzarc_Tool --extract ./Fld_AD_Town_map.lzarc ./Fld_AD_Town_map
-p/--pack: pack a directory into a new archive
Ex: Lzarc_Tool -p /Fld_TN_PostOffice_map/ ./Fld_TN_PostOffice_map_repack.lzarc
Ex: Lzarc_Tool --pack /Fld_TN_PostOffice_map/ ./Fld_TN_PostOffice_map_repack.lzarc
