


"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/crudHub").build();

connection.on("LoadAll", function (exceptPage) {

    if (window.location.pathname != exceptPage) {
        window.location.reload();
    }
});

connection.start().then().catch(function (err) {
    return console.log(err.toString());
});