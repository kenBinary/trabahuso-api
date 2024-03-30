require("dotenv").config();

const db = require("better-sqlite3")(process.env.SQLITE_DB_PATH, { fileMustExist: true });
db.pragma("journal_mode = WAL");

module.exports = db;