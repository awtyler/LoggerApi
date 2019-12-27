
var log = "default"
var logText = ""

$(document).ready(function() {
	toastr.options = {
		"closeButton": true,
		"debug": false,
		"newestOnTop": false,
		"progressBar": false,
		"positionClass": "toast-bottom-right",
		"preventDuplicates": false,
		"onclick": null,
		"showDuration": "300",
		"hideDuration": "1000",
		"timeOut": "2000",
		"extendedTimeOut": "1000",
		"showEasing": "swing",
		"hideEasing": "linear",
		"showMethod": "fadeIn",
		"hideMethod": "fadeOut"
	}				

	let queryparams = getUrlVars()
	if(queryparams["log"]) {
		log = queryparams["log"];
	}
	reloadLogContents();
});

// Read a page's GET URL variables and return them as an associative array.
function getUrlVars()
{
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for(var i = 0; i < hashes.length; i++)
    {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

var savedLogNames = ""

function addLogNameOption(item, value) {
	if(!value) value = item
	$("#logSelect").append(
		`<option value='${value}' ${value == log ? "SELECTED" : ""}>
			${item}
		</option>`);
}

function reloadLogNames() {
					$.get("/Log/LogNames", function(data, status) {
		var names = data.join("-");
		if(names != savedLogNames) {
			$("#logSelect").empty();
			
			var currentLogIncluded = false
			for(item of data) {
				addLogNameOption(item)
				currentLogIncluded = currentLogIncluded || item == log
			}

			if(!currentLogIncluded) {
				addLogNameOption(`${log} (removed)`, log)
			}

			if(savedLogNames != "") {
				toastr.success("New log names added.");
			}
		}

		savedLogNames = names;

	});
}

var reloadInterval;

function clearLog() {
	$.get(`/Log/Clear?log=${log}`, function() {
		reloadLogContents();
	});
}

function reloadLogContents() {
	//Check whether it's currently scrolled to the bottom of the log
	let logPre = document.getElementById("logContents");
	let isScrolledToBottom = logPre.scrollTop >= logPre.scrollHeight - logPre.clientHeight;
	$("#logContents").load(`/ShowLog?log=${log}`, function(data) {
		//Scroll to bottom of div
		if(isScrolledToBottom) {	//Scroll to bottom, if it was previously at the bottom
			logPre.scrollTop = logPre.scrollHeight;
		}
		logText = data;
		filterLog();
	})
	reloadLogNames();
}

function toggleReloadLogContents() {
	if(reloadInterval) {
		clearInterval(reloadInterval);
		reloadInterval = null;
		$("#reloadButton").html("Start Reload").addClass("btn-success").removeClass("btn-danger");
	} else {
		reloadInterval = setInterval(function() {
			reloadLogContents();
		}, 1000);
		$("#reloadButton").html("Stop Reload").addClass("btn-danger").removeClass("btn-success");
	}

}

function selectChanged(field) {
	log = field.value;
	reloadLogContents();
}

function filterLog(filter) {
	let logPre = $("#logContents");

	if(!filter || filter == "") {
		filter = $("#searchText").val()
	}

	filter = filter.toLowerCase ? filter.toLowerCase() : "";

	let text = logText

	if(!filter || filter == "") {
		logPre.html(text);
		return;
	}

	let tarray = text.split("\n");
	let output = []
	for(line of tarray) {
		if(line.toLowerCase().indexOf(filter) >= 0) {
			output.push(line);
		}
	}

	let outputText = output.join("\n");
	logPre.html(outputText);
}
