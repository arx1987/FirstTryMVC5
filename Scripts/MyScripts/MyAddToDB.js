document.getElementById("new_subject").addEventListener("click",
    function () {
        document.getElementById("add_input").style.display = "block";
        document.getElementById("dropdownMenu3").innerHTML = "Новая тема";
        //document.getElementById("DbNewSubject").autofocus = true;
    }, true);

document.getElementById("addToDB").addEventListener("click",
    function (e) {
        var subjectButton = document.getElementById("dropdownMenu3").innerHTML;
        if (subjectButton != "Новая тема" && subjectButton != "Выберите тему") {
            //добавляем значение в поле name=Subject
            document.getElementById("DbNewSubject").setAttribute("value", subjectButton);
        }
        //проверим заполнены ли поля Subject, Question, Answer перед отправкой формы на сервер
        //поле Question
        var question = document.getElementById("toBdQuestion").value;
        var answer = document.getElementById("toBdAnswer").value;
        var new_subject = document.getElementById("DbNewSubject").value;
        if ((question == "") || (answer == "") || (new_subject == "")) {//если одно из полей не заполнено - отмена отправки формы
            //нужно вывести сообщение, о том что "не все поля заполнены"
            var sendingResult = document.getElementById("sendingResult");
            sendingResult.innerHTML = "Не все поля заполнены"
            sendingResult.className += " colorRed";
            //отмена отправки формы!!!!
            e.preventDefault();
        }
    }, true);
