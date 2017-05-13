using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mPower.Tests.Environment;
using mPower.Framework;
using Moq;
using Paralect.Domain;

namespace mPower.Tests.MoqObjects.Common
{
    public class CommandServiceMock : IMock<CommandService>
    {
        private Mock<CommandService> _current;

        public List<ICommand> Commands { get; set; }

        public CommandServiceMock()
        {
            Commands = new List<ICommand>();
            _current = Create();
        }

        public Mock<CommandService> Create()
        {
            return new Mock<CommandService>(null, null, null);
        }

        public CommandService Object
        {
            get { return _current.Object; }
        }

        public CommandServiceMock SaveCommandsInAMock()
        {
            _current.Setup(x => x.Send(It.IsAny<ICommand>()))
                .Callback((ICommand[] cmds) => Commands.AddRange(cmds));
            _current.Setup(x => x.SendAsync(It.IsAny<ICommand>()))
                .Callback((ICommand[] cmds) => Commands.AddRange(cmds));
            
            return this;
        }
    }
}
