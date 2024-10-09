#include <uv.h>
#include <iostream>

using namespace std;

void on_new_connection(uv_stream_t* server, int status)
{
    if (status < 0)
    {
        cerr << "New connection error " << uv_strerror(status) << endl;
        return;
    }

    uv_tcp_t* client = (uv_tcp_t*)malloc(sizeof(uv_tcp_t));
    uv_tcp_init(uv_default_loop(), client);

    if (uv_accept(server, (uv_stream_t*)client) == 0)
    {
        cout << "New connection accepted" << endl;
        uv_read_start((uv_stream_t*)client, [](uv_handle_t* handle, size_t suggested_size, uv_buf_t* buf)
                      {
                          *buf = uv_buf_init((char*)malloc(suggested_size), suggested_size);
                      }, [](uv_stream_t* client, ssize_t nread, const uv_buf_t* buf)
                      {
                          if (nread > 0)
                          {
                              cout << "Received data: " << buf->base << endl;
                          }
                          else if (nread < 0)
                          {
                              if (nread != UV_EOF)
                              {
                                  cerr << "Read error " << uv_err_name(nread) << endl;
                              }
                              uv_close((uv_handle_t*)client, nullptr);
                          }

                          free(buf->base);
                      }
        );
    }
    else
    {
        uv_close((uv_handle_t*)client, nullptr);
    }
}

int main()
{
    uv_tcp_t server;
    uv_tcp_init(uv_default_loop(), &server);

    struct sockaddr_in addr;
    uv_ip4_addr("0.0.0.0", 7000, &addr);

    uv_tcp_bind(&server, (const struct sockaddr*)&addr, 0);
    int r = uv_listen((uv_stream_t*)&server, 128, on_new_connection);
    if (r)
    {
        cerr << "Listen error " << uv_strerror(r) << endl;
        return 1;
    }

    cout << "Server listening on port 7000" << endl;
    uv_run(uv_default_loop(), UV_RUN_DEFAULT);

    cout << "Server Start Finish!!!!" << endl;
    return 0;
}
