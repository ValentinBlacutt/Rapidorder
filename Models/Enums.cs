namespace Restaurante.Models
{
   
    public enum CategoriaProducto
    {
        Hamburguesa,
        Papas,
        Bebida,
        Combo
    }

    public enum EstadoPedido
    {
        Pendiente = 1,
        Confirmado = 2,
        EnPreparacion = 3,
        Listo = 4,
        Entregado = 5,
        Cancelado = 6
    }

    public enum Tamanio
    {
        Pequeno,
        Mediano,
        Grande
    }


    public enum TipoPedido
    {
        ParaComerAqui = 1,
        ParaLlevar = 2
    }

    
    public enum MetodoPago
    {
        PagoOnline = 1,
        PagoEnMostrador = 2
    }

  
    public enum EstadoPago
    {
        Pendiente = 1,
        Pagado = 2,
        Fallido = 3
    }

}
