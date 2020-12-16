;
window.onload = funcAddAttributeToBody;
function funcAddAttributeToBody() {
    //добавить тегу боди атрибут onresize="changeHeight"
    var h = document.getElementsByTagName("body")[0];
    var typ = document.createAttribute("onresize");
    typ.value = "changeHeight()";
    h.attributes.setNamedItem(typ);

    var x = document.getElementById("setHeight").offsetWidth;
    //нахдим все элементы и устанавливаем для них высоту
    var some = document.getElementsByClassName("chngHeight");
    for (var i = 0; i < some.length; i++) {
        some[i].style.height = x + "px";
        some[i].style.lineHeight = x + "px";
        //some[i].style.textDecoration = "none";
    }
}
function changeHeight() {
    //находим ширину элемента
    var x = document.getElementById("setHeight").offsetWidth;

    //нахдим все элементы и устанавливаем для них высоту
    var some = document.getElementsByClassName("chngHeight");
    for (var i = 0; i < some.length; i++) {
        some[i].style.height = x + "px";
    }
    /*const xx = document.querySelectorAll("span.a, span.c");
    for (const i = 0; i < xx.length; i++) {
        xx[i].style.color="red";
    }*/
    //document.getElementById("myBtn").style.width = "300px";
}
;