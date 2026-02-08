namespace FacturacionElectronica.Domain.ValueObjects
{
    public class Direccion
    {
        public string Calle { get; private set; }
        public string Numero { get; private set; }
        public string Complemento { get; private set; }
        public string Ciudad { get; private set; }
        public string Departamento { get; private set; }
        public string CodigoPostal { get; private set; }
        public string Pais { get; private set; }

        private Direccion() { }

        public Direccion(string calle, string numero, string ciudad, string departamento, string pais)
        {
            if (string.IsNullOrWhiteSpace(calle))
                throw new ArgumentException("La calle es requerida");
            if (string.IsNullOrWhiteSpace(ciudad))
                throw new ArgumentException("La ciudad es requerida");
            if (string.IsNullOrWhiteSpace(departamento))
                throw new ArgumentException("El departamento es requerido");
            if (string.IsNullOrWhiteSpace(pais))
                throw new ArgumentException("El país es requerido");

            Calle = calle;
            Numero = numero;
            Ciudad = ciudad;
            Departamento = departamento;
            Pais = pais;
        }

        public Direccion WithComplemento(string complemento)
        {
            Complemento = complemento;
            return this;
        }

        public Direccion WithCodigoPostal(string codigoPostal)
        {
            CodigoPostal = codigoPostal;
            return this;
        }

        public override string ToString()
        {
            return $"{Calle} {Numero} {Complemento}, {Ciudad}, {Departamento}, {Pais}";
        }
    }

    public class InformacionContacto
    {
        public string Telefono { get; private set; }
        public string Email { get; private set; }
        public string PaginaWeb { get; private set; }

        private InformacionContacto() { }

        public InformacionContacto(string telefono, string email)
        {
            if (string.IsNullOrWhiteSpace(telefono))
                throw new ArgumentException("El teléfono es requerido");
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email es requerido");

            Telefono = telefono;
            Email = email;
        }

        public InformacionContacto WithPaginaWeb(string paginaWeb)
        {
            PaginaWeb = paginaWeb;
            return this;
        }
    }

    public class ValorMonetario
    {
        public decimal Valor { get; private set; }
        public string Moneda { get; private set; }

        public ValorMonetario(decimal valor, string moneda = "COP")
        {
            if (valor < 0)
                throw new ArgumentException("El valor no puede ser negativo");
            if (string.IsNullOrWhiteSpace(moneda))
                throw new ArgumentException("La moneda es requerida");

            Valor = valor;
            Moneda = moneda;
        }

        public ValorMonetario Multiplicar(decimal factor)
        {
            return new ValorMonetario(Valor * factor, Moneda);
        }

        public ValorMonetario Sumar(ValorMonetario otro)
        {
            if (Moneda != otro.Moneda)
                throw new InvalidOperationException("No se pueden sumar valores en diferentes monedas");
            
            return new ValorMonetario(Valor + otro.Valor, Moneda);
        }
    }
}