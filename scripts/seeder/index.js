const axios = require('axios');

const addendum = '';

// Configuration
const CONFIG = {
  baseUrl: process.env.API_BASE_URL || 'http://localhost:5115',
  timeout: 30000
};

// Tech industry figures data
const USERS_DATA = [
  {
    username: 'linus_torvalds',
    email: 'linus@kernel.org',
    password: 'LinuxMaster1991',
    fullname: 'Linus Torvalds',
    biography: 'Creator of Linux kernel and Git version control system. Finnish-American software engineer who revolutionized open-source software development.'
  },
  {
    username: 'tim_berners_lee',
    email: 'tim@w3.org',
    password: 'WWW1989',
    fullname: 'Tim Berners-Lee',
    biography: 'Inventor of the World Wide Web, HTML, HTTP, and URLs. British computer scientist and founder of the World Wide Web Consortium (W3C).'
  },
  {
    username: 'ada_lovelace',
    email: 'ada@analytics.engine',
    password: 'FirstProgrammer1843',
    fullname: 'Ada Lovelace',
    biography: 'Often regarded as the first computer programmer. Wrote the first machine algorithm and envisioned computers beyond pure calculation.'
  },
  {
    username: 'alan_turing',
    email: 'alan@enigma.uk',
    password: 'TuringMachine1936',
    fullname: 'Alan Turing',
    biography: 'Father of theoretical computer science and artificial intelligence. Broke the Enigma code during WWII and created the Turing Test.'
  },
  {
    username: 'grace_hopper',
    email: 'grace@navy.mil',
    password: 'COBOL1959',
    fullname: 'Grace Hopper',
    biography: 'Pioneer of computer programming who developed the first compiler. Popularized machine-independent programming languages and coined the term "computer bug".'
  },
  {
    username: 'dennis_ritchie',
    email: 'dmr@bell-labs.com',
    password: 'CLangUNIX1972',
    fullname: 'Dennis Ritchie',
    biography: 'Creator of the C programming language and co-developer of the UNIX operating system. His work fundamentally shaped modern computing.'
  },
  {
    username: 'brian_kernighan',
    email: 'bwk@princeton.edu',
    password: 'HelloWorld1978',
    fullname: 'Brian Kernighan',
    biography: 'Co-author of "The C Programming Language" book and contributor to UNIX development. Known for his clear technical writing and programming philosophy.'
  },
  {
    username: 'guido_van_rossum',
    email: 'guido@python.org',
    password: 'PythonBDFL1989',
    fullname: 'Guido van Rossum',
    biography: 'Creator of Python programming language. Former Benevolent Dictator For Life (BDFL) of Python, now Distinguished Engineer at Microsoft.'
  },
  {
    username: 'brendan_eich',
    email: 'brendan@mozilla.org',
    password: 'JavaScript1995',
    fullname: 'Brendan Eich',
    biography: 'Creator of JavaScript programming language in just 10 days at Netscape. Co-founder of Mozilla and creator of the Brave browser.'
  },
  {
    username: 'margaret_hamilton',
    email: 'margaret@apollo.nasa.gov',
    password: 'Apollo111969',
    fullname: 'Margaret Hamilton',
    biography: 'Lead software engineer for NASA Apollo program. Coined the term "software engineering" and her code helped land humans on the moon.'
  }
];

// HTTP client setup
const apiClient = axios.create({
  baseURL: CONFIG.baseUrl,
  timeout: CONFIG.timeout,
  headers: {
    'Content-Type': 'application/json',
    'User-Agent': 'Microservices-API-Client/1.0'
  }
});

// Add request interceptor for logging
apiClient.interceptors.request.use(request => {
  console.log(`🚀 Making ${request.method?.toUpperCase()} request to: ${request.url}`);
  console.log('📦 Request data:', request.data);
  return request;
});

// Add response interceptor for logging
apiClient.interceptors.response.use(
  response => {
    console.log(`✅ Response ${response.status}: ${response.statusText}`);
    return response;
  },
  error => {
    console.log(`❌ Request failed: ${error.message}`);
    if (error.response) {
      console.log(`   Status: ${error.response.status}`);
      console.log(`   Data:`, error.response.data);
    }
    return Promise.reject(error);
  }
);

// Register user function
async function registerUser(userData) {
  try {
    const response = await apiClient.post('/api/auth/register', {
      username: userData.username + addendum, 
      email: userData.email,
      password: 'Password123!'
    });
    
    console.log(`✅ User ${userData.username} registered successfully`);
    return response.data.token; // Assuming the API returns a token
  } catch (error) {
    console.error(`❌ Failed to register user ${userData.username}:`, error.message);
    return null;
  }
}

// Update profile function
async function updateProfile(token, profileData) {
  try {
  // Now uses FormData
  const formData = new FormData();
  formData.append('name', profileData.fullname);
  formData.append('biography', profileData.biography);

    const response = await apiClient.put('/api/profile', formData, {
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'multipart/form-data'
      }
    });
    
    console.log(`✅ Profile updated successfully for ${profileData.fullname}`);
    console.log(response.data)
    return response.data;
  } catch (error) {
    console.error(`❌ Failed to update profile for ${profileData.fullname}:`, error.message);
    return null;
  }
}

// Main execution function
async function main() {
  console.log('🎯 Starting microservices API client...');
  console.log(`🌐 API Base URL: ${CONFIG.baseUrl}`);
  console.log('👥 Processing users from computer science and tech industry...\n');

  const results = {
    successful: 0,
    failed: 0,
    tokens: []
  };

  for (const userData of USERS_DATA) {
    console.log(`\n📋 Processing user: ${userData.username} (${userData.fullname})`);
    console.log(`📧 Email: ${userData.email}`);
    console.log(`📝 Bio: ${userData.biography.substring(0, 80)}...`);
    
    // Step 1: Register user
    const token = await registerUser(userData);
    
    if (token) {
      results.tokens.push({ username: userData.username, token });
      console.log(token)
      
      // Step 2: Update profile
      await updateProfile(token, {
        fullname: userData.fullname,
        biography: userData.biography
      });
      
      results.successful++;
    } else {
      results.failed++;
    }
    
    // Add small delay between requests to be nice to the API
    await new Promise(resolve => setTimeout(resolve, 500));
  }

  // Summary
  console.log('\n' + '='.repeat(60));
  console.log('📊 EXECUTION SUMMARY');
  console.log('='.repeat(60));
  console.log(`✅ Successful operations: ${results.successful}`);
  console.log(`❌ Failed operations: ${results.failed}`);
  console.log(`📋 Total users processed: ${USERS_DATA.length}`);
  
  if (results.tokens.length > 0) {
    console.log('\n🔑 Generated tokens:');
    results.tokens.forEach(({ username, token }) => {
      console.log(`   ${username}: ${token.substring(0, 20)}...`);
    });
  }
  
  console.log('\n🎉 Microservices API client execution completed!');
}

// Handle errors gracefully
process.on('unhandledRejection', (reason, promise) => {
  console.error('❌ Unhandled Rejection at:', promise, 'reason:', reason);
  process.exit(1);
});

process.on('uncaughtException', (error) => {
  console.error('❌ Uncaught Exception:', error);
  process.exit(1);
});

// Start the application
if (require.main === module) {
  main().catch(error => {
    console.error('❌ Application failed:', error);
    process.exit(1);
  });
}

module.exports = { registerUser, updateProfile, main };
