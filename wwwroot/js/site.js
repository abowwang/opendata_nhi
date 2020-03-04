// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// aysnc call API for GET
async function sendGetHttpRequest(_url){
    $.LoadingOverlay('show');  
    return await fetch(_url, {
        method: 'GET',
        cache: 'no-cache',
        headers: {
            'content-type': 'application/json; charset=utf-8',
        }
    }).then(resp => {
        return resp.json()
    }).then(respJson => {
        if (respJson.retCode != 0) {
            throw Error(respJson.retMessage);
        } 
        return respJson;
    }).catch(error => {
        console.error('Error:', error.message)
        return {
            retCode: 1,
            retMessage: error.message
        };
    }).then(response => {
        $.LoadingOverlay('hide',true);    
        return response;
    });    
}

// aysnc call API for POST
async function sendPostHttpRequest(_url,_body){
    $.LoadingOverlay('show');  
    return await fetch(_url, {
        method: 'POST',
        cache: 'no-cache',
        headers: {
            'content-type': 'application/json; charset=utf-8',
        },
        body: _body
    }).then(resp => {
        return resp.json()
    }).then(respJson => {
        if (respJson.retCode != 0) {
            throw Error(respJson.retMessage);
        } 
        return respJson;
    }).catch(error => {
        console.error('Error:', error.message)
        return {
            retCode: 1,
            retMessage: error.message
        };
    }).then(response => {
        $.LoadingOverlay('hide',true);    
        return response;
    });    
}

// aysnc call API for POST FORM
async function sendFormPostHttpRequest(_url,_body){
    $.LoadingOverlay('show');  
    return await fetch(_url, {
        method: 'POST',
        cache: 'no-cache',
        body: _body,
    }).then(resp => {
        return resp.json()
    }).then(respJson => {
        if (respJson.retCode != 0) {
            throw Error(respJson.retMessage);
        } 
        return respJson;
    }).catch(error => {
        console.error('Error:', error.message)
        return {
            retCode: 1,
            retMessage: error.message
        };
    }).then(response => {
        $.LoadingOverlay('hide',true);    
        return response;
    });    
}

// 產生 UUID 
function _uuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {var r = Math.random()*16|0,v=c=='x'?r:r&0x3|0x8;return v.toString(16);});
}


// 刷新 GeoCode
async function sendGeoCodeRequest(lat,long,key){
    return await fetch(`https://maps.googleapis.com/maps/api/geocode/json?latlng=${lat},${long}&sensor=false&key=${key}`, {
        method: 'GET',
        cache: 'no-cache'
    }).then(resp => {
        return resp.json()
    }).then(respJson => {
        return {
            retCode:0,
            retMessage: '',
            resp:respJson
        }
    }).catch(error => {
        console.error(`Error:${error}`)
        return {
            retCode: 1,
            retMessage: error
        };
    }).then(response => {
        
        return response;
    });   
}

// 取得現在位置，刷新地圖
function getLocation(){
    if(navigator.geolocation){
        // timeout at 60000 milliseconds (60 seconds)
        var options = {timeout:60000};
        navigator.geolocation.getCurrentPosition(DisplayGoogleMap, getLocationErrorHandler, options);
    } else{
            BootstrapDialog.show({
            message: "Sorry, browser does not support geolocation!"
        });
    }
}

  

function getLocationErrorHandler(error)
{
      console.error(`getCurrentPosition : code(${error.code}) message(${error.message})`);
}

function headElement(){
    var nameElement = document.createElement("td");
    nameElement.innerText = "醫事機構名稱";
    var telElement = document.createElement("td");
    telElement.innerText = "醫事機構電話";
    var addrElement = document.createElement("td");
    addrElement.innerText = "醫事機構地址";
    var humanElement = document.createElement("td");
    humanElement.innerText = "成人口罩剩餘數";
    var childElement = document.createElement("td");
    childElement.innerText = "兒童口罩剩餘數";
    var updatedElement = document.createElement("td");
    updatedElement.innerText = "來源資料時間";
    
    var trElement = document.createElement("tr");
    trElement.appendChild(nameElement);
    trElement.appendChild(telElement);
    trElement.appendChild(addrElement);
    trElement.appendChild(humanElement);
    trElement.appendChild(childElement);
    trElement.appendChild(updatedElement);
    return trElement;
}

function listElement(name,tel,addr,human,child,updated){
    var nameElement = document.createElement("td");
    nameElement.innerText = name;
    var telElement = document.createElement("td");
    telElement.innerText = tel;
    var addrElement = document.createElement("td");
    addrElement.innerText = addr;
    var humanElement = document.createElement("td");
    humanElement.innerText = human;
    var childElement = document.createElement("td");
    childElement.innerText = child;
    var updatedElement = document.createElement("td");
    updatedElement.innerText = updated;
    
    var trElement = document.createElement("tr");
    trElement.appendChild(nameElement);
    trElement.appendChild(telElement);
    trElement.appendChild(addrElement);
    trElement.appendChild(humanElement);
    trElement.appendChild(childElement);
    trElement.appendChild(updatedElement);
    return trElement;
}