require("dotenv").config();
const asyncHandler = require("express-async-handler");
const db = require("../helpers/dbConnection");

exports.getJobs = asyncHandler(async (req, res) => {
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
    const { month, year, province } = req.query;

    const statement = db.prepare(
      "SELECT job_title, location, salary, job_level, date_scraped FROM job_data order by date_scraped desc"
    );
    let jobs = statement.all();

    if (year) {
      jobs = jobs.filter((job) => {
        const jobYear = Number(job["date_scraped"].split("-")[0]);
        console.log(jobYear);
        return Number(year) === jobYear;
      });
    }

    if (month) {
      jobs = jobs.filter((job) => {
        const jobMonth = Number(job["date_scraped"].split("-")[1]);
        return months[month] === jobMonth;
      });
    }

    if (province) {
      jobs = jobs.filter((job) => {
        const jobLocation = job["location"].split(",")[0];
        return province.toUpperCase() === jobLocation.toUpperCase();
      });
    }
    res.status(200).json({
      data: jobs,
      count: jobs.length,
    });
  } catch (error) {
    res.status(400).send("error occured while retrieving jobs");
  }
});
