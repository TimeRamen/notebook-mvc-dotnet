// Write your JavaScript code.

var fromDate = document.getElementById("from");
var toDate = document.getElementById("to");
var today = new Date();
fromDate.valueAsDate = new Date(today.setMonth(today.getMonth()-1));

toDate.valueAsDate = new Date(today.setMonth(today.getMonth()+2));