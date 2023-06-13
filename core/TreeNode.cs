using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GameCore {
	/// <summary>
	/// Representa una estructura de datos de árbol de nodos
	/// </summary>
	/// <remarks>Créditos a: Ronnie Overby (https://stackoverflow.com/a/10442244)</remarks>
	/// <typeparam name="T"></typeparam>
	public class TreeNode<T> {
		/// <summary>
		/// Valor del nodo
		/// </summary>
		private readonly T value;
		/// <summary>
		/// Nodo padre de este nodo
		/// </summary>
		private readonly TreeNode<T> parent;
		/// <summary>
		/// Nodos hijo de este nodo
		/// </summary>
		private readonly List<TreeNode<T>> children;

		/// <inheritdoc cref="TreeNode{T}"/>
		/// <param name="value">Valor asociado a este nodo</param>
		/// <param name="parent">Nodo padre de este nodo</param>
		public TreeNode(T value, TreeNode<T> parent = null) {
			this.value = value;
			this.children = new List<TreeNode<T>>();
			this.parent = parent;
		}

		public TreeNode<T> this[int i] => children[i];

		public TreeNode<T> Parent => this.parent;

		public ReadOnlyCollection<TreeNode<T>> Children => this.children.AsReadOnly();

		public T Value => this.value;

		public TreeNode<T> AddChild(T value) {
			TreeNode<T> node = new TreeNode<T>(this.value, this);
			this.children.Add(new TreeNode<T>(value));
			return node;
		}

		public TreeNode<T>[] AddChildren(params T[] values) {
			return values.Select(this.AddChild).ToArray();
		}

		public TreeNode<T> GetChild(int i) {
			foreach(TreeNode<T> node in this.children)
				if(--i == 0)
					return node;

			return null;
		}

		public bool RemoveChild(TreeNode<T> node) {
			return children.Remove(node);
		}

		public void Traverse(Action<T> visitor) {
			visitor(this.value);
			foreach(TreeNode<T> child in this.children)
				child.Traverse(visitor);
		}
	}
}
