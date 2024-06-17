# MeBank
Тестовое задание для ТГК-2.

## Порядок установки.
Для установки необходимо выполнить следующие шаги:
1. Скопировать репозиторий.
2. Выполнить скрипт ```create.sql``` в базе данных ```postgresql```.
3. Выполнить команды в папке проекта (можно в терминале ```Visual Studio```):
```bash
npm i @quasar/extras
npm i @quasar/vite-plugin
npm i @vueuse/core
npm i apexcharts
npm i quasar
npm i sass
npm i vue-apexcharts
npm i vue-router
npm i vue3-apexcharts
```
4. Можно запускать проект.

## Нюансы
1. По умолчанию скрипт ```create.sql``` создаёт пользователя ```Server``` с паролем ```Server``` и даёт ему доступ к нужным таблицам и другим элеменам БД. При необходимости пользователя и пароль можно изменить.