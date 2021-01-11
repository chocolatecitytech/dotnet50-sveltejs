<script>
  import { stores } from "@sapper/app";
  const { session } = stores();
  import { onMount, onDestroy } from "svelte";
  const SESSION_TIME = 60 * 1000; //* 15
  const MODAL_RESPONSE_TIME = 60 * 1000; // * 2
  const IDLE_CHECK_UP = 30 * 1000;

  let idleCounter = 0;
  let showModal = false;
  let timeoutInt = -1;
  let intervalInt = -1;

  if ($session.user) {
    startSessionTracking();
  }
  function startSessionTracking() {
    intervalInt = setInterval(() => {
      if (idleCounter >= SESSION_TIME) {
        showModal = true;
        timeoutInt = timeoutInt(async () => {
          showModal = false;
          await logout();
        }, MODAL_RESPONSE_TIME);
      } else {
        idleCounter += 30 * 1000;
      }
    }, IDLE_CHECK_UP);
  }
  async function logout() {
    const result = await makeSessionCall("auth/logout");
    session.set({ user: null });
    cleanup();
  }
  async function keepSession() {
    const result = await makeSessionCall("auth/refresh");
    if (result.status == 200) {
      session.set({ user: await result.json() });
      cleanup();
    }
  }

  async function makeSessionCall(apiRoute) {
    return await fetch(apiRoute, {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    });
  }

  onDestroy(() => {
    cleanup();
    console.log("component is being destroyed");
  });
  onMount(() => {
    addEventListener("click", () => {
      idleCounter = 0;
    });
    console.log("component is being mounted");
  });
  function cleanup() {
    showModal = false;
    clearInterval(intervalInt);
    clearTimeout(timeoutInt);
  }
</script>

<!-- Modal -->
<div
  class="modal fade"
  class:show={showModal}
  id="staticBackdrop"
  data-bs-backdrop="static"
  data-bs-keyboard="false"
  tabindex="-1"
  aria-labelledby="staticBackdropLabel"
  aria-hidden={showModal ? "false" : "true"}
>
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="staticBackdropLabel">Attention</h5>
        <button
          type="button"
          class="btn-close"
          data-bs-dismiss="modal"
          aria-label="Close">X</button
        >
      </div>
      <div class="modal-body">
        Your session will expire soon. Please click "Keep Session" to stay
        logged in.
      </div>
      <div class="modal-footer">
        <button
          type="button"
          class="btn btn-secondary"
          data-bs-dismiss="modal"
          on:click={logout}>Log out</button
        >
        <button type="button" class="btn btn-primary" on:click={keepSession}
          >Keep Session</button
        >
      </div>
    </div>
  </div>
</div>

<style>
  .show {
    display: block;
  }
</style>
