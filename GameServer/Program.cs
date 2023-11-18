
using CS_SocketIO;
using GameServer;

const int SERVER_TIME_STEP = 100;
ServerUdp io = new ServerUdp(11000);
Game game = new Game();


io.On("connection",onConnection);


UpdateState();

void onConnection(object _client)
{
    Client client = (Client)_client;
    string username = ((dynamic)client.Data)?.Username;

    if (string.IsNullOrEmpty(username))
    {
        client.Disconnect("Debe enviar el username en la data inicial");
        return;
    }

    Console.WriteLine("Cliente conectado " + username);
    
    game.SpawnPlayer(client.Id,username);

    client.Emit("welcome", new {
            Message="Bienvenido al juego",
            Id = client.Id,
            State =game.State,
        });
    client.Broadcast("newPlayer",new { Id=client.Id , Username=username });
    client.On("move", (axis) => {
        int  horizontal = ((dynamic)axis).Horizontal;
        int vertical = ((dynamic)axis).Vertical;

        game.SetAxis(client.Id,new Axis {Horizontal=horizontal,Vertical = vertical  });
    });

    client.On("disparo", (Posiciondireccion) =>
     {
         Console.WriteLine("Cualquier cosa");
         float x = ((dynamic)Posiciondireccion).x;
         float y = ((dynamic)Posiciondireccion).y;
         float direcciony = ((dynamic)Posiciondireccion).direcciony;
         float direccionx = ((dynamic)Posiciondireccion).direccionx;
         Posiciondireccion posiciondireccion = new Posiciondireccion
         {
             this.x = x,
             this.y = y,
             this.direccionx = direccionx,
             this.direcciony = direcciony,
         };

         
         game.SpawnBullet(posiciondireccion);
     });
    client.On("rotacion", (Rotacion) =>
    {
        float x = ((dynamic)Rotacion).x;
        float y = ((dynamic)Rotacion).y;
        Rotacion rotacion = new Rotacion
        {
            this.x = x,
            this.y = y,
        };
        Player p = new Player();
        foreach (Player hola in game.State.Players)
        {
            if (Equals(player.Id, _client.Id))
            {
                p = player;
                break;
            }
        }
        game.SetRotatio(rotacion, p);
    });

    client.On("disconnect", (_client) =>
    {
        game.RemovePlayer(client.Id);

         Console.WriteLine("usuario desconectado " + client.Id);
    });
}
io.Listen();



 async Task UpdateState()
{
    while (true)
    {
        io.Emit("updateState", new { State = game.State });
        await Task.Delay(TimeSpan.FromMilliseconds(SERVER_TIME_STEP));
    }
}
