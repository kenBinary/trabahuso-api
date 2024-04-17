const { query, matchedData, param } = require("express-validator");
const asyncHandler = require("express-async-handler");
const db = require("../helpers/dbConnection");
const { getMedian } = require("../utils/statUtils");

exports.getJobLocations = [
  query("limit").isNumeric().escape().trim().toInt(),
  query("sort").isAlpha().escape().trim(),
  query("order").isAlpha().escape().trim(),
  asyncHandler(async (req, res) => {
    try {
      const { sort, order } = req.query;
      const validatedInputs = matchedData(req);
      const statement = db.prepare(
        "SELECT location, salary, date_scraped FROM job_data"
      );
      let jobs = statement.all();

      const locationData = {
        data: [],
        limitCount: 0,
        totalCount: 0,
      };

      const addedLocations = {};
      for (let i = 0; i < jobs.length; i++) {
        const province = jobs[i]["location"].split(",")[0];
        const salary = jobs[i]["salary"];

        if (province === "unspecified") {
          continue;
        }

        if (province in addedLocations) {
          addedLocations[province]["job count"] += 1;
          addedLocations[province]["median salary"].push(salary);
        } else {
          addedLocations[province] = {
            location: province,
            "job count": 1,
            "median salary": [salary],
          };
        }
      }

      const jLMedian = Object.values(addedLocations).map((jobLocation) => {
        return {
          ...jobLocation,
          "median salary": getMedian(jobLocation["median salary"]),
        };
      });
      locationData.data = [...jLMedian];

      locationData.totalCount = jLMedian.length;

      const sortType = sort === "jobCount" ? "job count" : "median salary";
      const orderType = order === "asc" ? order : "desc";

      if (sort) {
        switch (orderType) {
          case "asc":
            locationData.data.sort((a, b) => {
              return a[sortType] - b[sortType];
            });
            break;
          case "desc":
            locationData.data.sort((a, b) => {
              return b[sortType] - a[sortType];
            });
            break;
          default:
            break;
        }
      }

      if ("limit" in validatedInputs) {
        locationData.data = locationData.data.slice(0, validatedInputs.limit);
      }
      locationData.limitCount = locationData.data.length;

      res.status(200).json(locationData);
    } catch (error) {
      console.error(error);
      res.status(400).json({
        message: "bad request",
      });
    }
  }),
];

exports.getJobByProvince = [
  param("province").isString().trim().escape(),
  asyncHandler(async (req, res) => {
    try {
      // TODO: filter by date scraped
      let { province } = req.params;

      const wordSplit = province.split("-");
      if (wordSplit.length > 0) {
        province = wordSplit.join(" ");
      }

      let jobDetail = {
        location: "",
        jobCount: 0,
        medianSalary: 0,
      };

      const jobLocationQuery = db.prepare(
        "select location from job_data where SUBSTR(location,0, instr(location, ',')) = ?;"
      );
      const jobLocation = jobLocationQuery.get(province.toUpperCase());
      if (!jobLocation) {
        throw new Error(`not found`, { cause: `(${province}) not found` });
      }
      jobDetail.location = jobLocation["location"];

      const jobCountQuery = db.prepare(
        "select count(location) as job_count from job_data where SUBSTR(location,0, instr(location, ',')) = ?;"
      );
      let job = jobCountQuery.get(province.toUpperCase());
      jobDetail.jobCount = job.job_count;

      const jobSalariesQuery = db.prepare(
        "select salary from job_data where SUBSTR(location,0, instr(location, ',')) = ?;"
      );
      jobSalariesQuery.raw();
      const salaryList = jobSalariesQuery
        .all(province.toUpperCase())
        .flat()
        .sort();
      jobSalariesQuery.raw(false);

      if (salaryList.length === 2) {
        jobDetail.medianSalary = (salaryList[0] + salaryList[1]) / 2;
      } else {
        jobDetail.medianSalary =
          salaryList[Math.floor(salaryList.length - 1 / 2)];
      }

      res.status(200).json(jobDetail);
    } catch (error) {
      console.error(error.message);
      if (error.message === "not found") {
        res.status(404).json({
          message: error.cause,
        });
      } else {
        res.status(500).send("error");
      }
    }
  }),
];
