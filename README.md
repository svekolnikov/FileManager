# File manager
![Alt-FileManager](https://github.com/svekolnikov/FileManager/blob/master/FileManager/img/FileManager.jpg "Пример")

* Блок Path - отображает текущий путь к директории
* Блок Content - отображает таблицу подкаталогов и файлов внутри директории
* Блок System info - отображает сообщения об ошибках
* Блок Command line - ввод команд

# Команды 
| Ключ | Пример | Описание |
|------|--------|----------|
| .. |  | Переход на уровень выше |
| cd | cd C:\Source | Переход в указанную директорию |
| ls | ls C:\Source -p 2 | Вывод дерева файловой системы с условием “пейджинга” |
| cp | C:\source.txt D:\target.txt | Копирование файла |
| cp | C:\source D:\target | Копирование каталога рекурсивно |
| rm | rm C:\source.txt | Удаление файла |
| rm | rm C:\Source | Удаление каталога |
| nf | nf C:\source.txt | Создание файла |
| nd | nd C:\Source | Создание директории |
