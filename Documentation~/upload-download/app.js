var express = require("express");
var path = require("path");
var fs = require("fs");
var logger = require('morgan');

var app = express();
var root = path.join(process.cwd(), "public");

app.use(logger('dev'));
app.use(express.static(path.join(__dirname, 'public')));
app.listen(3000);

app.put("*", function (req, res) {
    var filePath = path.join(root, req.path);
    fs.mkdir(path.dirname(filePath), {recursive: true}, (err) => {
        if (err) {
            return console.error(err);
        }

        var ws = fs.createWriteStream(filePath);
        req.on("data", function (data) {
            ws.write(data);
        });

        req.on("end", function () {
            ws.end();
            res.send("success");
        });
    });
});

app.delete("*", function (req, res) {
    var dirPath = path.join(root, req.path);
	fs.exists(dirPath, function(exists) {
		if(exists == false) {
			res.send("fail");
			return;
		}	
	});
    fs.rmdir(dirPath, {recursive: true}, function (err) {
        if (err) {
            return console.error(err);
        }

        res.send("success");
    });
});

console.log("app start listance : 3000")