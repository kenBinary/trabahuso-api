const express = require("express");
const router = express.Router();
const techStackController = require("../controllers/techStackController");

// filter by "category"
router.get("/", techStackController.getTechFrequency);

module.exports = router;
