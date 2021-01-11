<script context="module">
  export async function preload(page, { user }) {
    if (user) {
      await this.fetch("auth/logout", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
      });
    }
  }
</script>

<script>
  import { goto, stores } from "@sapper/app";
  const { session } = stores();
  session.set({ user: null });
  goto("/");
</script>
