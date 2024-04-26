require("dotenv").config();
const createError = require("http-errors");
const express = require("express");
const cookieParser = require("cookie-parser");
const logger = require("morgan");
const helmet = require("helmet");
const cors = require("cors");
const jobsRouter = require("./routes/jobs");
const locationsRouter = require("./routes/locations");
const salaryRouter = require("./routes/salary");
const techStackRouter = require("./routes/techStack");
const config = require("config");

const app = express();

const corsOptions = {
  origin: config.get("cors.origin"),
  method: "GET",
};
app.use(cors(corsOptions));
app.use(helmet());
app.use(logger("dev"));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());

app.use("/api/jobs", jobsRouter);
app.use("/api/job-locations", locationsRouter);
app.use("/api/job-salaries", salaryRouter);
app.use("/api/tech-stack", techStackRouter);

// catch 404 and forward to error handler
app.use(function (req, res, next) {
  next(createError(404));
});

// error handler
// eslint-disable-next-line no-unused-vars
app.use(function (err, req, res, next) {
  res.status(err.status || 500).send("an error occured");
});

module.exports = app;
