<p>
    <h1 align="center">UserApiTestTaskVk</h1>
</p>

<p align="center">
    Web API для тестового задания на ASP.NET 6
</p>

<p align="center">
  <img src="https://img.shields.io/static/v1?label=&message=c%23&style=flat-square&color=0000ff"
      height="40">
  <img src="https://img.shields.io/badge/ASP.NET-purple?style=flat-square"
      height="40">
  <img src="https://img.shields.io/static/v1?label=&message=Entity-Framework&style=flat-square&color=blueviolet"
      height="40">
  <img src="https://img.shields.io/static/v1?label=&message=PostgreSql&style=flat-square&color=1A5276&logo=postgresql&logoColor=white"
      height="40">
  <img src="https://img.shields.io/static/v1?label=&message=Swagger&style=flat-square&color=green&logo=swagger&logoColor=white"
      height="40">
  <img src="https://img.shields.io/static/v1?label=&message=MediatR&style=flat-square&color=blue"
      height="40">
  <img src="https://img.shields.io/static/v1?label=&message=JWT&style=flat-square&color=BDB76B"
      height="40">
</p>

<div align="center">
    <a href="https://github.com/Skye7012/UserApiTestTaskVk/issues">
        <img src="https://img.shields.io/github/issues-raw/Skye7012/UserApiTestTaskVk" alt="Open Issues"/>
    </a>
    <a href="https://github.com/Skye7012/UserApiTestTaskVk/issues?q=is%3Aissue+is%3Aclosed">
        <img src="https://img.shields.io/github/issues-closed-raw/Skye7012/UserApiTestTaskVk" alt="Closed Issues"/>
    </a>
</div>



# Table Of Contents

- [ТЗ](#ТЗ)
- [Общее описание](#общее-описание)
- [Уточнения по реализации основных требований](#уточнения-по-реализации-основных-требований)
  - [Поле `user_password` сущности `user`](#Поле-user_password-сущности-user)
  - [Ограничение на одного администратора](#Ограничение-на-одного-администратора)
  - [Lock in SignUp](#Lock-in-SignUp)
  - [SoftDelete, Metadata](#SoftDelete-Metadata)
- [Уточнения по реализации опциональных требований:](#Уточнения-по-реализации-опциональных-требований)
    - [Авторизация](#Авторизация)
    - [Пагинация](#Пагинация)
    - [Unit-тесты](#Unit-тесты)
- [Локальный запуск](#локальный-запуск)



# ТЗ
![image](https://github.com/Skye7012/UserApiTestTaskVk/assets/86796337/0904c289-fc40-4e70-a80f-0ee83026b1c3)



# Общее описание
Реализован API на ASP.NET 6  
Реализованы основные и опциональные требования с уточнениями в реализации, описанными ниже  

Реализована поддержка docker-compose [(см. "Локальный запуск")](#локальный-запуск)  

API задокументирован с помощью `Swagger`  
Проект структурирован по принципам `clean architecture`  
Используется `CQRS` через [`MediatR`](https://github.com/jbogard/MediatR)  
В качестве ORM используется `Entity Framework Core`, в качестве СУБД `PostgreSql`  
Аутентификация реализована через [`JWT`](https://jwt.io/) токены  
  
Есть модульные и интеграционные тесты  
Тесты написаны с помощью `xUnit` и [`FluentAssertions`](https://github.com/fluentassertions/fluentassertions)  
Интеграционные тесты реализованы с помощью [`testcontainters`](https://github.com/testcontainers/testcontainers-dotnet) (и [`respawn`](https://github.com/jbogard/Respawn)) (поэтому нужен докер для их прогонки)  



# Уточнения по реализации основных требований:
###  Поле `user_password` сущности `user`  
- В сущности `user` столбец `user_password` разделён на `user_passwordHash` и `user_passwordSalt`, чтобы хранить пароли в БД в захэшированном виде  
  
### Ограничение на одного администратора
- Требование не иметь более одного пользователя с `user_group.code = "Admin"` реализовано путем инициализации пользователя-админа в БД автоматически ([см. DbInitExecutor.cs](src/UserApiTestTaskVk.Infrastructure/InitExecutors/DbInitExecutor.cs))

### Lock in SignUp
- Лок при регистрации с задержкой на 5 секунд реализован с помощью библиотеки [`DistributedLock`](https://github.com/madelson/DistributedLock)  
  Время задержки (5 секунд) может конфигурироваться через appSettings 

### SoftDelete, Metadata
- Было реализовано мягкое удаление для `Пользователя` через `BlockedUserState`, но также был реализован общий интерфейс `ISoftDeletable` для сущностей, поддерживающих мягкое удаление  
  Поле `created_date` так же было принято использовать для всех сущностей



# Уточнения по реализации опциональных требований:
###  Авторизация  
- Реализована не Basic-авторизация, а JWT-авторизация  

###  Пагинация  
- Реализована пагинация и сортировка для получения нескольких пользователей  

###  Unit-тесты  
- Написаны unit-тесты с помощью xUnit  



# Локальный запуск
- `git clone https://github.com/Skye7012/UserApiTestTaskVk.git`

- `cd UserApiTestTaskVk`

- `docker-compose build`

- `docker-compose up`

- **Swagger**: [http://localhost:5000/swagger/index.html](http://localhost:5000/swagger/index.html)

Volumes для БД будет создан на уровень выше корневой директории
