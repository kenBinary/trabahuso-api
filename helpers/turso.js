require("dotenv").config();
const { createClient } = require("@libsql/client");
const config = require("config");
const dbUrl = config.get("db.url");
const dbAuthToken = config.get("db.auth-token");

const client = createClient({
  url: dbUrl,
  authToken: dbAuthToken,
});

module.exports = client;
