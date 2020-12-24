# Лабораторная №4

Краткий обзор проектов, в том числе и из прошлых лабораторных:
- ***TestingTransferProtocol*** связывает все проекты и запускает все объекты; как и раньше, работает не службой, а в консоли для простоты тестирования;
- ***TransferProtocol*** содержит класс *DirectoryWatcher*, который и содержит логику наблюдения за директорией и перемещения файлов;
- ***CompressionUtils*** занимается шифрованием файла и последующим сжатием в архив, а также разархивированием и дешифровкой;
- ***ConfigurationManager*** занимается парсингом json и xml, доставляет данные в класс конфигурации;
- ***DataManager*** работает с базой данных и создаёт xml файлы с данными из базы;
- ***LogManager*** занимается логированием.

## Нововведения
- В проекте ***DataManager*** методы классов *DataManager*, *DatabaseManager* теперь асинхронные;
- Методы ***TransferProtocol*** тоже асинхронные(чтение, запись, передача файлов);
- Различные мелкие доработки.

## Ход работы
***TransferProtocol*** и ***DataManager*** работают независимо друг от друга в разных потоках. ***DataManager*** добавляет файлы в папку, которая находится под наблюдением ***TransferProtocol***, никак с ним не взаимодействуя; благодаря потоком они работают параллельно.
