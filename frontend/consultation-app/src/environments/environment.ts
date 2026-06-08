export const environment = {
  production: false,
  // Backend listens on HTTP 5266 and HTTPS 7060; use HTTP locally to avoid certificate issues
  apiUrl: 'http://localhost:5266',
  signalRUrl: 'http://localhost:5266/hubs/consultation'
};