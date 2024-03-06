module.exports = {
  content: [
    './Pages/**/*.cshtml',
    './Views/**/*.cshtml',
    './Areas/Identity/**/*.cshtml'
  ],
  theme: {
    extend: {
      colors: {
        'mainColor': '#225483',
        'btnColor': '#C35937',
        'btnHover': '#9F4529',
        'textColor': '#4b5563',
        'footerColor': '#31313B'
      },
      fontFamily: {
        'body': ['Kantumruy', 'sans-serif'],
        'heading': ['Karma', 'serif']
      },
    },
  },
  plugins: [],
}