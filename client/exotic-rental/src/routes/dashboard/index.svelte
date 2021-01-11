<script context="module">
  export async function preload(page, { user }) {
    if (!user) {
      this.redirect(302, "/login");
    }
  }
</script>

<script>
  import { stores } from "@sapper/app";
  const { session } = stores();
</script>

<div>
  {#if $session.user}
    <p>{JSON.stringify($session.user, null, 2)}</p>
  {/if}
</div>
