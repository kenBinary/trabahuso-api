const asyncHandler = require("express-async-handler");
const { query, matchedData } = require("express-validator");
const client = require("../helpers/turso");

exports.getJobs = [
  query("year").isInt().trim().escape(),
  query("month").isAlpha().trim().escape(),
  query("province").isString().trim().escape(),
  asyncHandler(async (req, res) => {
    try {
      const months = {
        jan: 1,
        feb: 2,
        mar: 3,
        apr: 4,
        may: 5,
        june: 6,
        july: 7,
        aug: 8,
        sept: 9,
        oct: 10,
        nov: 11,
        dec: 12,
      };
      const validatedData = matchedData(req);

      const statement = await client.execute(
        "SELECT job_title, location, salary, job_level, date_scraped FROM job_data order by date_scraped desc"
      );

      let jobs = statement.rows;

      if ("year" in validatedData) {
        jobs = jobs.filter((job) => {
          const jobYear = Number(job["date_scraped"].split("-")[0]);
          return Number(validatedData.year) === jobYear;
        });
      }

      if ("month" in validatedData) {
        jobs = jobs.filter((job) => {
          const jobMonth = Number(job["date_scraped"].split("-")[1]);
          return months[validatedData.month] === jobMonth;
        });
      }

      if ("province" in validatedData) {
        jobs = jobs.filter((job) => {
          const jobLocation = job["location"].split(",")[0];
          return (
            validatedData.province.toUpperCase() === jobLocation.toUpperCase()
          );
        });
      }

      res.status(200).json({
        data: jobs,
        count: jobs.length,
      });
    } catch (error) {
      console.error(error);
      res.status(400).json({
        message: "error occured while retrieving jobs",
      });
    }
  }),
];
