makemap
(c) mjt, 2019
Released under MIT-license.

Program creates random maze or loads bitmap files which contains map.
Exports maze to [filename].txt or out_[bitmapfilename].txt

Check exported .txt files, set modelfiles right at the beginning of the file.

With these .txt mapfiles one can convert maps to .obj file.


example 1:
makemap.exe -maze out.txt
(edit out.txt file)
makemap.exe -createmap out.txt
(import out.obj to blender)


example 2:
makemap.exe -bitmap floor.bmp wall.bmp
(edit out_floor.txt and out_wall.txt files)
makemap.exe -createmap out_floor.txt
makemap.exe -createmap out_wall.txt
(import *.obj files to blender)
