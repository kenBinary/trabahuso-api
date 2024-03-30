require("dotenv").config();

const sqlite3 = require("sqlite3").verbose();

const db = new sqlite3.Database(process.env.SQLITE_DB_PATH, (err) => {
    if (err == null) {
        console.log("database opened successfully");
    } else {
        console.log(err);
    }
});

module.exports = db;