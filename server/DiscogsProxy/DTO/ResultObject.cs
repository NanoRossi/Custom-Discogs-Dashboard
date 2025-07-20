using System;

namespace DiscogsProxy.DTO;

public class ResultObject<T>
{
    public ResultObject()
    {

    }

    public ResultObject(T content)
    {
        Result = content;
    }

    public ResultObject(Exception err)
    {
        Error = err;
    }

    public T? Result { get; set; }

    public Exception? Error { get; set; }

    public bool HasError { get => (Error != null); }
}