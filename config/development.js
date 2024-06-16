require("dotenv").config();
const development = {
  app: {
    port: 3000,
  },
  db: {
    url: `file:${process.env.SQLITE_DB_PATH}` || "file:./data/test.db",
    "auth-token": "",
  },
  cors: {
    origin: process.env.DEV_ORIGIN || "http://localhost:5173/",
  },
};
module.exports = development;
