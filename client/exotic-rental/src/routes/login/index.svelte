<script context="module">
  export async function preload(page, { user }) {
    if (user) {
      this.redirect(302, "/dashboard");
    }
  }
</script>

<script>
  let username = "";
  let password = "";
  let errors = [];
  let isSubmitting = false;
  async function handleSubmit() {
    isSubmitting = true;
    const response = await fetch("/login", {
      method: "POST",
      body: JSON.stringify({ username, password }),
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      credentials: "include",
    });
    isSubmitting = false;
    if (response.status === 200) {
      const result = await response.json();
      window.location.href = "/dashboard";
    } else {
      errors.push("Username and/or password is invalid");
    }
  }
</script>

<svelte:head>
  <title>Login | Exotic Rental</title>
</svelte:head>
<div class="row">
  <div class="col-sm-8 offset-sm-3">
    <div class="card" style="width: 18rem;">
      <div class="card-body">
        <form on:submit|preventDefault={handleSubmit}>
          <div class="form-group">
            <label for="inputUsername">Username</label>
            <input
              type="text"
              id="username"
              placeholder="username"
              bind:value={username}
              class="form-control"
            />
          </div>
          <div class="form-group">
            <label for="inputpassword">Password</label>
            <input
              type="password"
              id="password"
              bind:value={password}
              class="form-control"
            />
          </div>
          <button
            type="submit"
            class="btn btn-primary mt-2"
            disabled={isSubmitting}
          >
            Login
          </button>
          {#if errors && errors.length}
            {#each errors as error}
              <p class="alert alert-danger">{error}</p>
            {/each}
          {/if}
        </form>
      </div>
    </div>
  </div>
</div>
