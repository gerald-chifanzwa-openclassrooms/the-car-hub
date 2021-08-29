module.exports = {
    purge: {
        enabled: true,
        content: ['./Views/**/*.cshtml', './**/*.js'],
    },
    mode: 'JIT',
    darkMode: false, // or 'media' or 'class'
    theme: {
        extend: {},
    },
    variants: {
        extend: {
            ringColor: ['focus-visible'],
            ringWidth: ['focus-visible']
        },
    },
    plugins: [
        require('@tailwindcss/forms'),
        require('@tailwindcss/typography')
    ],
}
