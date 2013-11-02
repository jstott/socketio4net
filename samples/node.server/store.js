var trunk = {};

module.exports = {
  version: '0.1.2',

  /**
   * Store data in a closure to be called or withdraw later.
   * @this {store}
   * @param {String} key Data key.
   * @param {String|Number|Array|Object|Function} value Data value.
   * @returns {this} Return `this` to enable chaining.
   * @example
   *
   *     var store = require( 'store' );
   *     store.set( 'age', 17 );
   *     store.set( 'name', 'ben' );
   *     store.set('gender','male').set('weight',150);
   */
  set: function (key, value) {
    // data can be overwritten
    trunk[key] = value;

    return this;
  },
  /**
 * Get stored data.
 * @this {store}
 * @param {String} key Data key.
 * @returns {data|false} Return the stored data if available.
 * @example
 *
 *     var store = require( 'secret' );
 *     store.get( 'age' );
 */
  get: function (key) {
    return trunk[key] || false;
  },
  /**
 * Increment stored data.
 * @method incr
 * @param {String} key Data key.
 * @returns {number} Incremented value
 * @example
 *
 *     var store = require( 'store' );
 *     store.incr( 'count' );
 */
  incr: function (key, fn) {
    var val = trunk[key] || 0;
    trunk[key] = ++val;
    if (fn) {
      fn();
    }
    return val;
  },

  incrby: function (key, value) {
    var val = trunk[key] || 0;
    val = val + (value || 0);
    trunk[key] = val;
    
    return val;
  },
  /**
  * Decrement stored data
  * @method decr
  * @param {string} key Data key
  */
  decr: function (key, fn) {
    var val = trunk[key] || 0;

    trunk[key] = --val;
    if (fn) {
      fn();
    }
    return val;
  },
  /**
  * Decrement stored data by value
  @method decrby
  @param {string} key Data key
  @param {number} value value to decrement
  */
  decrby: function (key, value) {
    var val = trunk[key] || 0;
    val = val - (value || 0);
    trunk[key] = val;
    return val;
  },
  /**
 * Remove stored data.
 * @method remove
 * @param {String} key Data key.
 * @returns {this} Return `this` to enable chaining.
 * @example
 *
 *     var store = require( 'store' );
 *     store.remove( 'age' );
 */
  remove: function (key) {
    delete trunk[key];

    return this;
  }
};