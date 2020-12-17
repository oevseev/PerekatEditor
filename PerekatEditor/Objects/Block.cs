using System;
using System.Text;

namespace PerekatEditor.Objects
{
    /// <summary>
    /// Блок уровня
    /// </summary>
    class Block
    {
        protected Block() {}

        /// <summary>
        /// Создает блок с заданным именем.
        /// </summary>
        /// <param name="tileId">Имя блока</param>
        public Block(int blockId, string blockName)
        {
            BlockId = blockId;
            BlockName = blockName;
        }

        /// <summary>
        /// Имя блока
        /// </summary>
        public string BlockName { get; set; }

        /// <summary>
        /// ID блока
        /// </summary>
        public int BlockId { get; set; }

        /// <summary>
        /// Создает новый блок, исходя из вертикали, на которой он расположен, и идентификатора блока.
        /// </summary>
        /// <param name="blockId">Идентификатор блока</param>
        /// <param name="verticalIndex">Индекс вертикали</param>
        /// <returns></returns>
        public static Block Construct(int blockId, int verticalIndex)
        {
            string parity = (verticalIndex % 2 == 0) ? "Even" : "Odd";
            string blockName = Converters.BlockConverter.GetBlockName(blockId);

            if (blockName == null)
                return new Block(0, null);

            // Не проще ли было просто сконкатенировать строку на месте?
            StringBuilder fullBlockName = new StringBuilder();
            fullBlockName.Append(parity);
            fullBlockName.Append(blockName);

            return new Block(blockId, fullBlockName.ToString());
        }
    }
}
