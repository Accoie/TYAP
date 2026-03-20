using System;
using System.Collections.Generic;
using System.Text;

namespace Ast;

public abstract class AstNode
{
    public abstract void Accept( IAstVisitor visitor );
}
