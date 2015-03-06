# Port Listener

This demo how to recive data from optional port. Port listening functions excluded in stand alone class *PereodicPortListener*.

```sh
PereodicPortListener(int pNumber, int lPer, Func<Socket, int> pFunc)
```
**pNumber** - number of port, which we want to listen.

**lPer** - period of listening in milliseconds.

**pFunc** - function that parse recived from Socket
