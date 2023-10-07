
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

    //Disable all btn css class
    $(".btn").addClass("disabled");

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
const connectionHubUrl = new URL('messageRelayHub', window.location.protocol + '//' + window.location.host);
//const connectionHubUrl = new URL('messageRelayHub', 'https://localhost:7155');
//const connectionHubUrl = new URL('messageRelayHub', 'https://gptcustomerservice.azurewebsites.net/');

const connection = new signalR.HubConnectionBuilder().withUrl(connectionHubUrl.href).withAutomaticReconnect().withHubProtocol(new signalR.JsonHubProtocol()).build();
connection.maximumParallelInvocationsPerClient = 10;


connection.on("ReceiveMessageUpdate", function (messageResponse) {
    if (messageResponse.state === "End") {
        var converter = new showdown.Converter({ tables: 'true' }),
            text = notifications.innerHTML,
            html = converter.makeHtml(text);

        notifications.innerHTML = html;

        notifications = null;

        //enable all btn css class
        $(".btn").removeClass("disabled");
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