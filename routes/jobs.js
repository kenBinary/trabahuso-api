const express = require("express");
const router = express.Router();
const jobsController = require("../controllers/jobsController");

// TODO: filter by province, month, year
// TODO: pagination
router.get("/", jobsController.getJobs);

module.exports = router;
