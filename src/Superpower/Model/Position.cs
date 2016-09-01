namespace Superpower.Model
{
    public struct Position
    {
        public int Absolute { get; }
        public int Line { get; }
        public int Column { get; }

        public Position(int absolute, int line, int column)
        {
            Absolute = absolute;
            Line = line;
            Column = column;
        }

        public static Position Zero { get; } = new Position(0, 1, 1);

        public static Position Empty { get; } = default(Position);

        public Position Advance(char overChar)
        {
            if (overChar == '\n')
                return new Position(Absolute + 1, Line + 1, 1);

            return new Position(Absolute + 1, Line, Column + 1);
        }

        public override string ToString()
        {
            return $"{Absolute} (line {Line}, column {Column})";
        }
    }
}