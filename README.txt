Замечания:
0) Все пути к папкам и файлам должны быть либо полными, либо они будут искаться начиная с текущей папки
1) команда help показывает все команды
2) все команды при вызове help сначала написано смысл команды через много пробелов как вызывать, почти у каждого вызова есть параметры они вводятся через пробел в квадратных скобка поясняется что как должен выглядеть параметр если такое требуется, звездой помечены необязательные параметры.
3) getdrives на маке показывает совсем другое и это норм
4) gotodir нужно ввести либо полный путь к директории, в противном случае поиск директории будет произведен из текущей
5) gfd выводит файлы из ТЕКУЩЕЙ директории по ОБЯЗАТЕЛЬНОЙ глубине погружения(0-на текущем уровне, 1-на текущем и на уровень ниже) и маски по желанию вида .разрешение(.txt, .png and e.t.c.)
6) sfc показывает содержимое ТЕКСТОВОГО файла, второй параметр - кодировка файла, третий параметр - кодировка вывода(По дефолту всё в UTF8)
7) copyall ввод директории источника и конечной директории аналогичны: либо полный, либо поиск из текущей, маска обязательная, параметр перезаписи необязательный
   Если файл будет копироваться, который уже есть в директории, то выйдет сообщение об этом, но ВСЕ ОСТАЛЬНОЕ НОРМ СКОПИРУЕТСЯ без ошибок
8) create  В имени разрешение указывать не надо, если такой файл есть он будет пересоздан, после вызова команды будет Ввод текста, чтобы закончить нажмите на стрелочку вверх на клавиатуре!!!!
9) subfiles суммируются в первый указанный файл
10) diff работает как в википедии указано, но немного глупее, то есть то что можно заменить сложные случае он просто удаляет и добавляет(Но работает и показывает разницу)
11) Методы со switch  короче 40 строк нельзя было сделать

0) All paths to folders and files must either be complete, or they will be searched starting from the current folder
1) the help command shows all commands
2) all commands when calling help, the meaning of the command is first written through many spaces how to call, almost every call has parameters they are entered through a space in a square bracket, it is explained what the parameter should look like if this is required, optional parameters are marked with a star.
3) getdrives on the mac shows something completely different and this is normal
4) gotodir must be entered either the full path to the directory, otherwise the directory will be searched from the current one
5) gfd outputs files from the CURRENT directory according to the REQUIRED immersion depth (0-at the current level, 1-at the current and lower level) and masks at the request of the view.resolution(.txt, .png and e.t.c.)
6) sfc shows the contents of a TEXT file, the second parameter is the encoding of the file, the third parameter is the encoding of the output (By default everything is in UTF8)
7) copyall the input of the source directory and the destination directory are similar: either full or search from the current one, the mask is mandatory, the rewrite parameter is optional
If a file is copied that is already in the directory, a message will be sent about it, but EVERYTHING ELSE will be COPIED without errors.
8) create permission is not necessary to specify in the name, if there is such a file, it will be recreated, after calling the command there will be text input, to finish click on the up arrow on the keyboard!!!!
9) subfiles are summed up in the first specified file
10) diff works as indicated in wikipedia, but a little more stupid, that is, what can be replaced in the case it simply removes and adds (But it works and shows the difference)
11) Methods with switch shorter than 40 lines could not be done
