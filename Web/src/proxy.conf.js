const API_TARGET = {
  "target": "http://172.17.0.1:5115",
  "secure": false
};

// const TARGET = {
//   "target": "http://localhost:5189",
//   "secure": false
// };

// const TARGET = {
//   "target": "http://localhost:5111",
//   "secure": false
// };

const PROXY_CONFIG = {
  "/weatherforecast": API_TARGET,
  "/api/": API_TARGET,

  // "/api/userinfo": API_TARGET,
  // "/api/login": API_TARGET,
  // "/api/logout": API_TARGET,
  // "/api/register": API_TARGET,


  // Profile
  // "/api/profile": API_TARGET,

  // Users
  // "/api/user/": API_TARGET,
  // "/api/search-users": API_TARGET,
  // "/api/top-users": API_TARGET,

  // Avatar
  // "/api/avatars/": API_TARGET,
};

module.exports = PROXY_CONFIG;
