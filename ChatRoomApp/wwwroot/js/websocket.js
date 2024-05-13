function socko() {
   
    const userName = "@Model.UserName"; // Replace with the user's registered name
    socket = new WebSocket("ws://localhost:7055/chat?userName=" + encodeURIComponent(userName));
    socket.onmessage = function (event) {
        const message = JSON.parse(event.data);
        const chatMessages = document.getElementById("chatMessages");
        chatMessages.innerHTML += `<div class="message">
                                    <span class="user">${message.userName}</span>
                                    <span class="text">${message.message}</span>
                                </div>`;
        chatMessages.scrollTop = chatMessages.scrollHeight;
    };

    function sendMessage() {
        const messageInput = document.getElementById("messageInput");
        const message = messageInput.value.trim();
        if (message !== "") {
            socket.send(JSON.stringify({ userName: userName, message: message }));
            messageInput.value = "";
        }
    }
}