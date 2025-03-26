"use strict";

// SignalR connection setup
var connection = new signalR.HubConnectionBuilder().withUrl("/crudHub").build();

// Auto refresh function
function refreshDashboard() {
    if (window.location.pathname.includes("/Admin/Dashboard")) {
        window.location.reload();
    }
}

// Set up auto refresh every 10 seconds
setInterval(refreshDashboard, 10000);

// SignalR event handler for real-time updates
connection.on("LoadAll", function (exceptPage) {
    if (window.location.pathname.includes("/Admin/Dashboard") && window.location.pathname != exceptPage) {
        window.location.reload();
    }
});

// Start SignalR connection
connection.start().then(function() {
    console.log("Dashboard auto-refresh initialized");
}).catch(function (err) {
    console.error(err.toString());
});
