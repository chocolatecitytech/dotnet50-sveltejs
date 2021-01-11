<script context="module">
	export async function preload(page, session) {
		// return this.fetch(`blog.json`).then(r => r.json()).then(posts => {
		// 	return { posts };
		// });
		const apiRoute = "http://localhost:5000/api/blogs";
		const response = await this.fetch(apiRoute);
		const posts = await response.json();
		return {
			posts,
		};
	}
</script>

<script>
	export let posts;
</script>

<style>
	ul {
		margin: 0 0 1em 0;
		line-height: 1.5;
	}
</style>

<svelte:head>
	<title>Blog</title>
</svelte:head>

<h1>Recent posts</h1>

<ul>
	{#each posts as post}
		<!-- we're using the non-standard `rel=prefetch` attribute to
				tell Sapper to load the data for the page as soon as
				the user hovers over the link or taps it, instead of
				waiting for the 'click' event -->
		<li><a rel="prefetch" href="blog/{post.slug}">{post.title}</a></li>
	{/each}
</ul>
