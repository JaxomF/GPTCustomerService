
function SpinOn($spin, $result) {
    $spin.removeClass("invisible");
    $spin.css("display", "inline-block");
    $result.addClass("invisible");
    $result.css("display", "none");
}

function OnlySpinOn($spin) {
    $spin.removeClass("invisible");
    $spin.css("display", "inline-block");
}

function OnlySpinOff($spin) {
    $spin.addClass("invisible");
    $spin.css("display", "none");
}

function SpinOff($spin, $result, data) {
    $spin.addClass("invisible");
    $spin.css("display", "none");
    if (data !== null) {
        $result.html(data);
    }
    $result.removeClass("invisible");
    $result.css("display", "inline-block");
}

function GetView($spin, $result, $url, $data) {
    //SpinOn($spin, $result);
    OnlySpinOn($spin);
    if ($url === undefined) {
        $url = $result.data("url")
    }

    var data;
    //if ($data !== undefined) {
    //    data = $data.text();
    //}

    $.ajax({
        type: "Post",
        data: {
            texte: $data,
        },
        url: $url,
        cache: false
    })
        .done(function (result) {
            //SpinOff($spin, $result, result);            
        }).fail(function (jqXHR, textStatus, errorThrown) {
            //SpinOff($spin, $result, ErrorMessage("Error : " + textStatus + "<br/>" + errorThrown));
        });

}


function GetViewLoad($spin, $result, $url, $data) {
    SpinOn($spin, $result);

    if ($url === undefined) {
        $url = $result.data("url")
    }

    var data;
    if ($data !== undefined) {
        data = $data.text();
    }

    $.ajax({
        type: "Post",
        data: {
            texte: data,
        },
        url: $url,
        cache: false
    })
        .done(function (result) {
            SpinOff($spin, $result, result);
        }).fail(function (jqXHR, textStatus, errorThrown) {
            SpinOff($spin, $result, ErrorMessage("Error : " + textStatus + "<br/>" + errorThrown));
        });

}

function onBegin() {
    if ($("#frm").valid()) {
        SpinOn($("#spinResult"), $("#result"))
    };
}

function onComplete(response) {
    SpinOff($("#spinResult"), $("#result"), response.responseText);
};


//add code to recevie notification from SignalR hub
//const connectionHubUrl = new URL('messageRelayHub', 'https://localhost:7155');
const connectionHubUrl = new URL('messageRelayHub', 'https://gptcustomerservice.azurewebsites.net/');

const connection = new signalR.HubConnectionBuilder().withUrl(connectionHubUrl.href).withAutomaticReconnect().withHubProtocol(new signalR.JsonHubProtocol()).build();

connection.on("ReceiveMessageUpdate", function (messageResponse) {
    if (messageResponse.state === "End") {
        var converter = new showdown.Converter({ tables: 'true' }),
            text = notifications.innerHTML,
            html = converter.makeHtml(text);

        notifications.innerHTML = html;

        if (messageResponse.whatAbout === "Mesure") {
            var json = JSON.parse(messageResponse.content);

            notifications.innerHTML = json.bodyPart;
            //switch human body part for selct on svg
            switch (notifications.innerText) {
                case "shoes":
                    $("#left-foot").removeClass("left-foot");
                    $("#left-foot").addClass("selectHB");
                    $("#right-foot").removeClass("right-foot");
                    $("#right-foot").addClass("selectHB");
                    break;
                case "chest":
                    $("#chest").removeClass("chest");
                    $("#chest").addClass("selectHB");
                    break;
                case "shoulder":
                    $("#left-shoulder").removeClass("left-shoulder");
                    $("#left-shoulder").addClass("selectHB");
                    $("#right-shoulder").removeClass("right-shoulder");
                    $("#right-shoulder").addClass("selectHB");
                    break;
                case "leg":
                    $("#left-leg").removeClass("left-leg");
                    $("#left-leg").addClass("selectHB");
                    $("#right-leg").removeClass("right-leg");
                    $("#right-leg").addClass("selectHB");
                    break;
                case "arm":
                    $("#left-arm").removeClass("left-arm");
                    $("#left-arm").addClass("selectHB");
                    $("#right-arm").removeClass("right-arm");
                    $("#right-arm").addClass("selectHB");
                    break;
                case "waist":
                    $("#waist").removeClass("waist");
                    $("#waist").addClass("selectHB");
                    break;
            }
          
            var divinfos = document.getElementById("infos");
            var p = document.createElement("p");
            p.innerHTML = json.bodyPart + "<br/>" + "Taille : " + json.size;
            divinfos.appendChild(p);
        }

        notifications = null;
    }
    else {

        if (messageResponse.state === "Start") {
            div = $("#spin" + messageResponse.whatAbout);
            document.getElementById('result' + messageResponse.whatAbout).innerHTML = "";
        }
        else {
            if (div !== null) {
                OnlySpinOff(div);
            }
            if (messageResponse.content !== null) {
                notifications.innerHTML = messageResponse.content;
            }
        }
    }

});

connection.start().then(function () {
    console.log("connected");
}).catch(function (err) {
    return console.error(err.toString());
});