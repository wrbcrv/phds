/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      fontFamily: {
        suiGenesis: ['"Sui Genesis"', 'sans-serif'],
        robotoMono: ['"Roboto Mono"']
      }
    },
  },
  plugins: [],
}
