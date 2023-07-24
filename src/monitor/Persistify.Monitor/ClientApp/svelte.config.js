import adapter from '@sveltejs/adapter-static';

/** @type {import('@sveltejs/kit').Config} */
const config = {
	kit: {
		adapter: adapter({assets: 'build', pages: 'build', fallback: null})

	}
};

export default config;
