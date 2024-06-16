require("dotenv").config();

const production = {
  app: {
    port: process.env.PORT,
  },
  db: {
    url: process.env.TURSO_DATABASE_URL || "",
    "auth-token": process.env.TURSO_AUTH_TOKEN || "",
  },
  cors: {
    origin: process.env.PROD_ORIGIN || "http://localhost:5173/",
  },
};
module.exports = production;
